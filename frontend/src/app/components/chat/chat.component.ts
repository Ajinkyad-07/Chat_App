import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth.service';
import { MessageComponent } from '../message/message.component';
import { ChatInputComponent } from '../chat-input/chat-input.component';
import { Message } from '../../models/message.model';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, MessageComponent, ChatInputComponent],
  template: `
    <div class="chat-container">
      <header class="chat-header">
        <h2>Chat Room</h2>
        <button (click)="logout()" class="logout-btn">
          Logout
        </button>
      </header>
      
      <main class="messages-container" #messagesContainer>
        <div class="messages-list">
          <app-message
            *ngFor="let message of messages; trackBy: trackByMessageId"
            [message]="message"
            [isCurrentUser]="message.userId === currentUserId"
          ></app-message>
        </div>
      </main>
      
      <app-chat-input (send)="onSendMessage($event)"></app-chat-input>
    </div>
  `,
  styles: [`
    .chat-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
      background-color: #f5f5f5;
    }
    .chat-header {
      padding: 1rem;
      background-color: #007bff;
      color: white;
      display: flex;
      justify-content: space-between;
      align-items: center;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .messages-container {
      flex: 1;
      overflow-y: auto;
      padding: 1rem;
      scroll-behavior: smooth;
    }
    .messages-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
    .logout-btn {
      padding: 0.5rem 1rem;
      background-color: transparent;
      border: 1px solid white;
      color: white;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.2s;
    }
    .logout-btn:hover {
      background-color: rgba(255,255,255,0.1);
    }
    @media (max-width: 768px) {
      .chat-header {
        padding: 0.75rem;
      }
      .chat-header h2 {
        font-size: 1.2rem;
      }
      .messages-container {
        padding: 0.75rem;
      }
    }
  `]
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  
  messages: Message[] = [];
  currentUserId = '';
  private subscriptions: Subscription[] = [];
  private shouldScrollToBottom = true;

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.subscriptions.push(
      this.chatService.messages$.subscribe(messages => {
        this.messages = messages;
        this.shouldScrollToBottom = true;
      }),
      this.authService.user$.subscribe(user => {
        if (user) {
          this.currentUserId = user.uid;
        } else {
          this.router.navigate(['/login']);
        }
      })
    );
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
    }
  }

  private scrollToBottom(): void {
    try {
      const container = this.messagesContainer.nativeElement;
      container.scrollTop = container.scrollHeight;
      this.shouldScrollToBottom = false;
    } catch (err) {
      console.error('Error scrolling to bottom:', err);
    }
  }

  trackByMessageId(index: number, message: Message): string {
    return message.id;
  }

  onSendMessage(text: string) {
    if (!this.currentUserId) return;
    
    this.chatService.sendMessage({
      text,
      userId: this.currentUserId,
      username: 'User'
    });
  }

  async logout() {
    try {
      await this.authService.logout();
      this.chatService.disconnect();
      this.router.navigate(['/login']);
    } catch (error) {
      console.error('Logout error:', error);
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.chatService.disconnect();
  }
}