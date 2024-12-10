import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Message } from '../models/message.model';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private socket: WebSocket | null = null;
  private messagesSubject = new BehaviorSubject<Message[]>([]);
  messages$ = this.messagesSubject.asObservable();

  constructor() {
    this.connectToWebSocket();
  }

  private connectToWebSocket(): void {
    const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
    const websocketUrl = `${protocol}//${window.location.hostname}:5110`; // Adjust to match your server port
    debugger
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
        timestamp: new Date(message.timestamp)
      }]);
    };

    this.socket.onerror = (error: Event) => {
      console.error('WebSocket error:', error);
    };

    this.socket.onclose = (event: CloseEvent) => {
      console.warn('WebSocket connection closed:', event.reason);
    };
  }

  sendMessage(message: Omit<Message, 'id' | 'timestamp'>): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      this.socket.send(JSON.stringify({
        ...message,
        timestamp: new Date()
      }));
    } else {
      console.error('WebSocket is not connected');
    }
  }

  disconnect(): void {
    if (this.socket) {
      this.socket.close();
    }
  }
}
