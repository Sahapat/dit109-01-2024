from typing import Annotated
from fastapi import FastAPI, Depends, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from fastapi.security import HTTPBasic, HTTPBasicCredentials

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'],
    allow_credentials=True,
    allow_methods=['*'],
    allow_headers=['*']
)
security = HTTPBasic()

class User(BaseModel):
    id: int
    name: str
    password: str

class UserManager():
    def __init__(self):
        self.runningNumber = 0
        self.users = []
    def register(self, name, password):
        self.runningNumber = self.runningNumber + 1
        self.users.append(User(
            id=self.runningNumber,
            name=name,
            password=password
        ))
    def login(self, username, password):
        for user in self.users:
            if user.name == username and user.password == password:
                return user
        return False
class Choice(BaseModel):
    choice: str

class RegisterUser(BaseModel):
    username: str
    password: str

userManager = UserManager()
room = [
    {
        "name": "TestRoom",
        "player1": "",
        "player2": "",
        "player1Choice": "",
        "player2Choice": ""
    }
]

def login(credential: Annotated[HTTPBasicCredentials, Depends(security)]):
    user = userManager.login(username=credential.username, password=credential.password)
    if user:
        return user
    else:
        raise HTTPException(status_code=400, detail="ข้อมูล User ไม่ถูกต้อง")

@app.get("/user/all", dependencies=[Depends(login)])
def all_user():
    return userManager.users

@app.post("/user/register")
def register_user(registerUser: RegisterUser):
    userManager.register(
        name=registerUser.username,
        password=registerUser.password
    )
    return "สำเร็จ"

@app.post("/join_room/{is_player1}")
def join_room(user: Annotated[User, Depends(login)], is_player1: bool):
    if is_player1:
        room[0]['player1'] = user.name
    else:
        room[0]['player2'] = user.name
    return 'สำเร็จ'

@app.get("/room_state")
def get_room_state():
    return room[0]

@app.post("/send_choice")
def send_choice(choice: Choice, user: Annotated[User, Depends(login)]):
    if room[0]['player1'] == user.name:
        room[0]['player1Choice'] = choice.choice
    elif room[0]['player2'] == user.name:
        room[0]['player2Choice'] = choice.choice
    else:
        return 'หาผู้เล่นไม่เจอ'
    
    return 'สำเร็จ'

@app.post('/check_winner')
def check_winner():
    p1 = room[0]['player1Choice']
    p2 = room[0]['player2Choice']

    if p1 == p2:
        return 'เสมอ'
    
    if  (p1 == 'กรรไกร' and p2 == 'กระดาษ') or \
        (p1 == '' and p2 == '') or \
        (p1 == '' and p2 == ''):
        return 'ผู้เล่น 1 ชนะ'
    else:
        return 'ผู้เล่น 2 ชนะ'

@app.post('/reset_room')
def reset_room():
    room[0]['player1Choice'] = ''
    room[0]['player2Choice'] = ''
    return 'สำเร็จ'