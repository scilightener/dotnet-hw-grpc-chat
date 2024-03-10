import './App.css';
import { ChatClient } from './protos/chat_grpc_web_pb';
import { JwtServiceClient } from './protos/get_jwt_grpc_web_pb';
import { useState } from "react";
import { Request } from "./protos/get_jwt_pb";
import { ChatMessageRequest } from "./protos/chat_pb";
import google_protobuf_empty_pb from 'google-protobuf/google/protobuf/empty_pb'


const chatClient = new ChatClient('http://localhost:8080');
const jwtClient = new JwtServiceClient('http://localhost:8080');

function App() {
  const [userName, setUserName] = useState('');
  const [accessToken, setAccessToken] = useState('');
  const [joined, setJoined] = useState(false);
  const [chatMessages, setChatMessages] = useState([]);
  const [userMessage, setUserMessage] = useState('');
  
  const messages = [];
  
  const authorize = () => {
    const request = new Request();
    request.setUsername(userName);
    jwtClient.getJwt(request, {}, (err, response) => {
      if (err) {
        console.log('err: ', err);
        return;
      }
      setAccessToken(response.toObject().token);
    });
  };
  
  const loadHistory = () => {
    const metadata = { 'Authorization': `Bearer ${accessToken}` };
    chatClient.joinChat(new google_protobuf_empty_pb.Empty(), metadata, (err, response) => {
      if (err) {
        console.log('err: ', err);
        return;
      }
      const history = response.toObject().messagesList;
      console.log(history);
      messages.push(...history);
      setChatMessages([...messages]);
    });
  };
  
  const joinChat = () => {
    setJoined(true);
    loadHistory();
    receiveMessages();
  };
  
  const receiveMessages = () => {
    const metadata = { 'Authorization': `Bearer ${accessToken}` };
    const responseStream = chatClient.startReceivingMessages(new google_protobuf_empty_pb.Empty(), metadata);
    responseStream.on("data", (response) => {
      const dto = response.toObject();
      console.log(dto);
      messages.push(dto);
      setChatMessages([...messages]);
    });
    
    responseStream.on("end", () => {
      console.log('bye');
    });
  };
  
  const sendMessage = () => {
    const request = new ChatMessageRequest();
    request.setContent(userMessage);
    const metadata = { 'Authorization': `Bearer ${accessToken}` };
    chatClient.sendChatMessage(request, metadata, (err) => {
      if (err) {
        console.log('err: ', err);
      }
    });
  };
  
  return (
    <>
      <div>
        <label>
          Enter your name:
          <input
            type="text"
            name="username"
            onChange={(e) => setUserName(e.target.value)}
          />
        </label>
        <button onClick={authorize}>Authorize</button>
        {accessToken && <button onClick={joinChat}>Join chat</button>}
      </div>
      {joined && <div>
        <label>
          Enter your message:
          <input
            type="text"
            name="chat-message"
            onChange={(e) => setUserMessage(e.target.value)}
          />
        </label>
        <button onClick={sendMessage}>Send message</button>
      </div>}
      {joined && <div>
        <p>Chat:</p>
        <div>
          { chatMessages && chatMessages.map((m, i) => (
            <ChatMessage username={m.user} message={m.content} key={`message-${i}`} />
          )) }
        </div>
      </div>}
    </>
  )
}

const ChatMessage = ({ username, message }) => {
  return (
    <p>{username}: {message}</p>
  )
};

export default App;
