import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth.service';
import { Message } from '../../models/message.model';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-chat',
  templateUrl: 'chat.component.html',
  styleUrl: 'chat.component.scss'
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  
  messages: Message[] = [];
  user!: User;
  receiverUserId = '';
  receiverUserName = '';
  private subscriptions: Subscription[] = [];
  private shouldScrollToBottom = true;

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit() {
    this.subscriptions.push(
      this.chatService.messages$.subscribe(messages => {
        this.messages = messages;
        this.shouldScrollToBottom = true;
      })
    );
    this.authService.user$.subscribe(user => {
      if (user) {
        this.user = user;
      } else {
        this.router.navigate(['/login']);
      }
    })
    // Fetch the query parameter (uId) from the route
    this.activatedRoute.queryParams.subscribe(params => {
      this.receiverUserId = params['uId'];
      this.receiverUserName = params['userName'];
    });
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
    return message.Id;
  }

  onSendMessage(text: string) {
    if (!this.user.Uid && !this.receiverUserId) return;
      let message : Message = {
        Id: "",
        Text: text,
        SenderName: this.user.DisplayName,
        SenderId: this.user.Uid,
        Timestamp: new Date(),
        ReceiverId: this.receiverUserId,
        ReceiverName: this.receiverUserName
      }
    this.chatService.sendMessage(message);
  }


  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.chatService.disconnect();
  }
}