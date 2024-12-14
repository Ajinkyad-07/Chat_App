import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Message } from '../../models/message.model';
import { formatMessageDate } from '../../utils/date.util';

@Component({
  selector: 'app-message',
  templateUrl: 'message.component.html',
  styleUrl: 'message.component.scss'
})
export class MessageComponent {
  @Input() message!: Message;
  @Input() isCurrentUser = false;

  formatDate(date: Date): string {
    return formatMessageDate(date);
  }
}