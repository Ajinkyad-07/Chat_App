import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Message } from '../../models/message.model';
import { formatMessageDate } from '../../utils/date.util';

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="message"
      [class.sent]="isCurrentUser"
      [class.received]="!isCurrentUser"
    >
      <div class="message-content">
        <span class="username" [class.sent-username]="isCurrentUser">
          {{isCurrentUser ? 'You' : message.username}}
        </span>
        <p class="text">{{message.text}}</p>
        <span class="timestamp">{{formatDate(message.timestamp)}}</span>
      </div>
    </div>
  `,
  styles: [`
    .message {
      max-width: 70%;
      margin: 0.5rem 0;
      padding: 0.5rem 1rem;
      border-radius: 1rem;
      animation: fadeIn 0.3s ease-in;
    }
    .sent {
      align-self: flex-end;
      background-color: #007bff;
      color: white;
    }
    .received {
      align-self: flex-start;
      background-color: white;
      box-shadow: 0 1px 2px rgba(0,0,0,0.1);
    }
    .message-content {
      position: relative;
    }
    .username {
      font-size: 0.8rem;
      opacity: 0.8;
      margin-bottom: 0.2rem;
      display: block;
    }
    .sent-username {
      text-align: right;
    }
    .text {
      margin: 0;
      word-wrap: break-word;
    }
    .timestamp {
      font-size: 0.7rem;
      opacity: 0.6;
      display: block;
      margin-top: 0.3rem;
      text-align: right;
    }
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
    @media (max-width: 768px) {
      .message {
        max-width: 85%;
      }
    }
  `]
})
export class MessageComponent {
  @Input() message!: Message;
  @Input() isCurrentUser = false;

  formatDate(date: Date): string {
    return formatMessageDate(date);
  }
}