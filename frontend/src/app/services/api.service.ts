import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { Login, LoginRequest } from '../models/login.model';

@Injectable({
    providedIn: 'root'  // This makes the service available throughout the application
})
export class ApiService {
    private apiUrl = `http://localhost:5110/api`;

    constructor(private http: HttpClient) { }

    // GET request to fetch data
    getUsers(): Observable<User[]> {
        return this.http.get<User[]>(`${this.apiUrl}/User/all`);
    }

    // GET request for a single user by ID
    getUserById(uid: number): Observable<User> {
        return this.http.get<User>(`${this.apiUrl}/User/${uid}`);
    }

    // POST request to create a new user
    registerUser(user: User): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/User/register`, user);
    }

    // PUT request to update an existing user
    editUser(uid: number, user: any): Observable<User> {
        return this.http.put<User>(`${this.apiUrl}/User/edit/${uid}`, user);
    }

    // DELETE request to remove a user
    deleteUser(uid: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/User/delete/${uid}`);
    }

    // POST request to create a new user
    loginUser(request: LoginRequest): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/User/login`, request);
    }
}


