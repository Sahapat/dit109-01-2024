import aiosqlite
from uuid import uuid4
from typing import Annotated, List, Dict
from fastapi import FastAPI, Depends, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, computed_field
from fastapi.security import HTTPBasic, HTTPBasicCredentials, APIKeyHeader

DATABASE_URL = '/mnt/c/Users/InspireTale/Desktop/database.db'

async def init_db():
    async with aiosqlite.connect(DATABASE_URL) as db:
        await db.execute("""
        CREATE TABLE IF NOT EXISTS User (
            id TEXT PRIMARY KEY,
            username TEXT,
            password TEXT
        )
        """)
        await db.execute("""
        CREATE TABLE IF NOT EXISTS Leaderboard(
            user_id TEXT,
            point INTEGER
        )
        """)
        await db.commit()

async def get_db():
    async with aiosqlite.connect(DATABASE_URL) as db:
        db.row_factory = aiosqlite.Row  # Enable row access by column name
        yield db

class User(BaseModel):
    id: str
    username: str
    password: str

class UserResponse(BaseModel):
    id: str
    username: str

class LeaderboardResponse(BaseModel):
    username: str
    point: str

class UserManager():
    async def all_users(self, db: aiosqlite.Connection):
        query = 'SELECT id, username FROM User'
        async with db.execute(query) as cr:
            rows = await cr.fetchall()
            users = []
            for row in rows:
                users.append(UserResponse(**dict(row)))
            return users
    
    async def register(self, db: aiosqlite.Connection, username, password):
        query = 'INSERT INTO User(id, username, password) VALUES (?, ?, ?)'
        user_id = str(uuid4())
        async with db.execute(query, (user_id, username, password)) as cr:
            await db.commit()
        leaderboard_query = 'INSERT INTO Leaderboard(user_id, point) VALUES(?, ?)'
        async with db.execute(leaderboard_query, (user_id, 0)):
            await db.commit()
        async with db.execute('SELECT id, username FROM User WHERE id = ?', (user_id, )) as cr:
            row = await cr.fetchone()
            return UserResponse(**dict(row))

    async def login(self, db: aiosqlite.Connection, username, password):
        query = 'SELECT id, username FROM User WHERE username = ? and password = ?'
        async with db.execute(query, (username, password)) as cr:
            row = await cr.fetchone()
            if row is None:
                return False
            return UserResponse(**dict(row))

    async def add_point(self, db: aiosqlite.Connection, user_id):
        query = 'UPDATE Leaderboard SET point = point + ? WHERE user_id = ?'
        async with db.execute(query, (10, user_id)) as cr:
            await db.commit()

    async def leaderboard(self, db: aiosqlite.Connection):
        query = '''
            SELECT username, point FROM User u
            JOIN Leaderboard l ON u.id = l.user_id
            ORDER BY l.point desc
        '''
        async with db.execute(query) as cr:
            rows = cr.fetchall()
            records = []
            for row in rows:
                records.append(LeaderboardResponse(**dict(row)))
            return records

rules = {"hammer": "scissors", "scissors": "paper", "paper": "hammer"}
class Room(BaseModel):
    id: str
    player1: str
    player2: str
    p1_choice: str
    p2_choice: str
    is_close: bool

    @computed_field
    @property
    def is_game_finish(self) -> bool:
        return self.p1_choice != '' and self.p2_choice != ''

    @computed_field()
    @property
    def is_game_start(self) -> bool:
        return self.player1 != '' and self.player2 != ''
    
    @computed_field()
    @property
    def winner(self) -> int:
        p1 = self.p1_choice
        p2 = self.p2_choice
        if p1 == p2:
            return 0
        return 1 if rules.get(p1, '') == p2 else 2 

class Choice(BaseModel):
    choice: str

class RegisterUser(BaseModel):
    username: str
    password: str

userManager = UserManager()
rooms: List[Room] = []
room_idx_by_id: Dict[str, str] = {}

def verify_api_key(api_key: str = Depends(APIKeyHeader(name='x-api-key'))):
    if api_key != '0':
        raise HTTPException(status_code=401, detail="API Key ไม่ถูกต้อง")
app = FastAPI(dependencies=[Depends(verify_api_key)])
app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'],
    allow_credentials=True,
    allow_methods=['*'],
    allow_headers=['*']
)
security = HTTPBasic()

@app.on_event('startup')
async def startup():
    await init_db()

async def login(credential: Annotated[HTTPBasicCredentials, Depends(security)], db = Depends(get_db)):
    user = await userManager.login(db=db, username=credential.username, password=credential.password)
    if user:
        return user
    else:
        raise HTTPException(status_code=400, detail="ข้อมูล User ไม่ถูกต้อง")

@app.get("/users", dependencies=[Depends(login)])
async def all_user(db = Depends(get_db)):
    users = await userManager.all_users(db=db)
    return users

@app.post("/user")
async def register_user(registerUser: RegisterUser, db=Depends(get_db)):
    user = await userManager.register(
        db=db,
        username=registerUser.username,
        password=registerUser.password
    )
    return user

@app.get("/rooms")
def get_rooms(user: Annotated[User, Depends(login)]):
    return rooms

@app.post("/room")
def create_room(user: Annotated[User, Depends(login)]):
    room = Room(
        id=str(uuid4()),
        player1=user.id,
        player2='',
        p1_choice='',
        p2_choice='',
        is_close=False
    )
    rooms.append(room)
    room_idx_by_id[room.id] = len(rooms) - 1
    return room

@app.post("/room/join/{room_id}")
def join_room(room_id: str, user: Annotated[User, Depends(login)]):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
        raise HTTPException(404, 'ไม่พบห้อง')
    
    if rooms[room_idx].player2 == '':
        rooms[room_idx].player2 = user.id
        return 'สำเร็จ'
    raise HTTPException(400, 'ห้องเต็ม')

@app.post("/room/leave/{room_id}")
def leave_room(room_id: str, user: Annotated[User, Depends(login)]):
    room_idx = room_idx_by_id.get(room_id, -1)
    room: Room = rooms[room_idx]
    if room_idx == -1:
       raise HTTPException(404, 'ไม่พบห้อง') 
    if room.player1 == user.id:
        rooms.pop(room_idx)
    elif room.player2 == user.id:
        rooms[room_idx].player2 = ''
    
@app.get("/room/{room_id}")
def get_room_state(room_id: str):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
        raise HTTPException(404, 'ไม่พบห้อง')
    return rooms[room_idx]

@app.post("/room/send_choice/{room_id}")
def send_choice(room_id: str, choice: Choice, user: Annotated[User, Depends(login)]):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
        raise HTTPException(404, 'ไม่พบห้อง')
    
    room: Room = rooms[room_idx]
    if room.player1 == user.id:
        rooms[room_idx].p1_choice = choice.choice
    elif room.player2 == user.id:
        rooms[room_idx].p2_choice = choice.choice
    else:
        raise HTTPException(404, 'ไม่พบผู้เล่น')
    
    return 'สำเร็จ'

@app.post('/room/check_winner/{room_id}')
async def check_winner(room_id: str, user: Annotated[User, Depends(login)], db = Depends(get_db)):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
        raise HTTPException(404, 'ไม่พบห้อง')
    
    room: Room = rooms[room_idx]
    room.is_close = True
    result = room.winner
    if result == 1:
        if not room.is_close:
            await userManager.add_point(db=db, user_id=room.player2)
        return 'Player 2 Win'
    elif result == 2:
        if not room.is_close:
            await userManager.add_point(db=db, user_id=room.player1)
        return 'Player 1 Win'

    return 'Draw'
