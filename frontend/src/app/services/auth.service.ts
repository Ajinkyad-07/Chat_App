import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Login, LoginRequest } from '../models/login.model';
import { User } from '../models/user.model';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userSubject = new BehaviorSubject<User | null>(null);
  user$: Observable<User | null> = this.userSubject.asObservable();

  constructor(private api: ApiService) {}

  async login(email: string, password: string) {
    try {
      const request: LoginRequest = {
        Email: email,
        Password: password
      };
      return this.api.loginUser(request).subscribe(
        (data: User) => {
          console.log("Login Details...", data);
          localStorage.setItem("userId", data.Uid);
          this.userSubject.next(data);
        },
        (error) => {
          console.error('Error fetching users:', error);  // Handle error
        }
      );
      // const result = await signInWithEmailAndPassword(this.auth, email, password);
      // localStorage.setItem("userId", result.user.uid);
      // return result;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  async register(email: string, password: string, userName: string) {
    try {
      const request: User = {
        Uid: "",
        Email: email,
        Password: password,
        DisplayName: userName
      };
      return this.api.registerUser(request).subscribe(
        (data: User) => {
          console.log("Register Details...", data);
          return data;
        },
        (error) => {
          console.error('Error fetching users:', error);  // Handle error
        }
      );
      // const result = await createUserWithEmailAndPassword(this.auth, email, password);
      // return result;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  async logout() {
    localStorage.removeItem("userId");
  }

}