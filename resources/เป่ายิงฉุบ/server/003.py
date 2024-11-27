from uuid import uuid4
from typing import Annotated, List, Dict
from fastapi import FastAPI, Depends, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, computed_field
from fastapi.security import HTTPBasic, HTTPBasicCredentials


class User(BaseModel):
    id: str
    name: str
    password: str

class UserManager():
    def __init__(self):
        self.users = []
    def register(self, name, password):
        self.users.append(User(
            id=str(uuid4()),
            name=name,
            password=password
        ))
    def login(self, username, password):
        for user in self.users:
            if user.name == username and user.password == password:
                return user
        return False

rules = {"ค้อน": "กรรไกร", "กรรไกร": "กระดาษ", "กระดาษ": "ค้อน"}
class Room(BaseModel):
    id: str
    player1: str
    player2: str
    p1_choice: str
    p2_choice: str

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
        return 1 if rules[p1] == p2 else 2 

class Choice(BaseModel):
    choice: str

class RegisterUser(BaseModel):
    username: str
    password: str

userManager = UserManager()
rooms: List[Room] = []
room_idx_by_id: Dict[str, str] = {}

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'],
    allow_credentials=True,
    allow_methods=['*'],
    allow_headers=['*']
)
security = HTTPBasic()

def login(credential: Annotated[HTTPBasicCredentials, Depends(security)]):
    user = userManager.login(username=credential.username, password=credential.password)
    if user:
        return user
    else:
        raise HTTPException(status_code=400, detail="ข้อมูล User ไม่ถูกต้อง")

@app.get("/users", dependencies=[Depends(login)])
def all_user():
    return userManager.users

@app.post("/user")
def register_user(registerUser: RegisterUser):
    userManager.register(
        name=registerUser.username,
        password=registerUser.password
    )
    return "สำเร็จ"

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
        p2_choice=''
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

@app.delete("/room/{room_id}")
def delete_room(room_id: str, user: Annotated[User, Depends(login)]):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
       raise HTTPException(404, 'ไม่พบห้อง') 
    rooms.pop(room_idx)
    
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
def check_winner(room_id: str, user: Annotated[User, Depends(login)]):
    room_idx = room_idx_by_id.get(room_id, -1)
    if room_idx == -1:
        raise HTTPException(404, 'ไม่พบห้อง')
    
    room: Room = rooms[room_idx]
    result = room.winner
    if result == 1:
        return 'ผู้เล่น 2 ชนะ'
    elif result == 2:
        return 'ผู้เล่น 1 ชนะ'

    return 'เสมอ'
