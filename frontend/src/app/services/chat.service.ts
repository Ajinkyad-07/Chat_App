import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Message } from '../models/message.model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private socket: WebSocket | null = null;
  private messagesSubject = new BehaviorSubject<Message[]>([]);
  private userId: any;
  messages$ = this.messagesSubject.asObservable();

  constructor() {
    this.userId = localStorage.getItem("userId");
    this.connectToWebSocket();
  }

  private connectToWebSocket(): void {
    const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
    const websocketUrl = `${protocol}//${window.location.hostname}:5110?userId=${this.userId}`; // Adjust to match your server port
    
    // Initialize the WebSocket
    this.socket = new WebSocket(websocketUrl);

    // Set up WebSocket event listeners
    this.socket.onopen = () => {
      console.log('Connected to WebSocket server');
    };

    this.socket.onmessage = (event: MessageEvent) => {
      const message: Message = JSON.parse(event.data);
      const currentMessages = this.messagesSubject.value;
      this.messagesSubject.next([...currentMessages, {
        ...message,
        Timestamp: new Date(message.Timestamp)
      }]);
    };

    this.socket.onerror = (error: Event) => {
      console.error('WebSocket error:', error);
    };

    this.socket.onclose = (event: CloseEvent) => {
      console.warn('WebSocket connection closed:', event.reason);
    };
  }

  sendMessage(message: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      let id = "bVVXES6y7AY9RljPKsIFEFWAzVM2";
      let msg : Message = {
        Text : message,
        UserName: "user",
        Timestamp: new Date(),
        UserId : id,
        Id : this.generateRandomId()
      }
      this.socket.send(JSON.stringify(msg));
    } else {
      console.error('WebSocket is not connected');
    }
  }

  disconnect(): void {
    if (this.socket) {
      this.socket.close();
    }
  }

  generateRandomId(): string {
    const length = 10
    const characters = 'abcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        const randomIndex = Math.floor(Math.random() * characters.length);
        result += characters[randomIndex];
    }
    return result;
  }
}
