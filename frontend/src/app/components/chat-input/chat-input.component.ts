import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="input-container">
      <input
        type="text"
        [(ngModel)]="message"
        (keyup.enter)="sendMessage()"
        placeholder="Type a message..."
        class="message-input"
      />
      <button (click)="sendMessage()" [disabled]="!message.trim()" class="send-btn">
        Send
      </button>
    </div>
  `,
  styles: [`
    .input-container {
      padding: 1rem;
      background-color: white;
      display: flex;
      gap: 1rem;
      border-top: 1px solid #eee;
      position: sticky;
      bottom: 0;
    }
    .message-input {
      flex: 1;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 1rem;
    }
    .message-input:focus {
      border-color: #007bff;
      outline: none;
    }
    .send-btn {
      padding: 0.75rem 1.5rem;
      background-color: #007bff;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-weight: 500;
      transition: background-color 0.2s;
    }
    .send-btn:disabled {
      background-color: #cccccc;
      cursor: not-allowed;
    }
    .send-btn:hover:not(:disabled) {
      background-color: #0056b3;
    }
    @media (max-width: 768px) {
      .input-container {
        padding: 0.75rem;
      }
      .message-input {
        padding: 0.5rem;
      }
      .send-btn {
        padding: 0.5rem 1rem;
      }
    }
  `]
})
export class ChatInputComponent {
  @Output() send = new EventEmitter<string>();
  message = '';

  sendMessage() {
    if (this.message.trim()) {
      this.send.emit(this.message);
      this.message = '';
    }
  }
}