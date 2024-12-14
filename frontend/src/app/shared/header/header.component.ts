import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

    constructor(
      private chatService: ChatService,
      private authService: AuthService,
      private router: Router
    ) {}

    async logout() {
      try {
        await this.authService.logout();
        this.chatService.disconnect();
        localStorage.removeItem("userId");
        this.router.navigate(['/login']);
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
}
