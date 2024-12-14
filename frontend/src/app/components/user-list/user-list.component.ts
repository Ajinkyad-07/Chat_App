import { Component } from '@angular/core';
import { User } from '../../models/user.model';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent {
  users: User[] = [];

  constructor(private api: ApiService,
    private router: Router
  ) {
    this.getAllUsers();
  }

  getAllUsers()
  {
    this.api.getUsers().subscribe((users: User[]) => {
      const userId = localStorage.getItem("userId");
      this.users = users.filter(u => u.Uid != userId);
    },
    (error) => {
      console.error('Error fetching users:', error);  // Handle error
    }
    )
  }

  // Method to handle the click event
  onUserClick(user: User): void {
    console.log('User clicked:', user);
    this.router.navigate(['/chat'], { queryParams: { uId: user.Uid } });
  }

  trackByUid(index: number, user: User): string {
    return user.Uid;
  }
}
