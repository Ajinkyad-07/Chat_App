import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-chat-input',
  templateUrl: 'chat-input.component.html',
  styleUrl: 'chat-input.component.scss'
})
export class ChatInputComponent {
  @Output() send = new EventEmitter<string>();
  public message = '';

  sendMessage() {
    if (this.message.trim()) {
      this.send.emit(this.message);
      this.message = '';
    }
  }
}