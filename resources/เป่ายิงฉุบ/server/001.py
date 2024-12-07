from fastapi import FastAPI
from pydantic import BaseModel

app = FastAPI()

class Choice(BaseModel):
    player: str
    choice: str

room = [
    {
        "name": "TestRoom",
        "player1Choice": "",
        "player2Choice": ""
    }
]

@app.get("/room_state")
def get_room_state():
    return room[0]

@app.post("/send_choice")
def send_choice(choice: Choice):
    if choice.player == 'player1':
        room[0]['player1Choice'] = choice.choice
    elif choice.player == 'player2':
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