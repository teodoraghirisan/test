import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Users } from '../models/users';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
    // private usersSubject: BehaviorSubject<any>;
    public users: any;

    constructor(private http: HttpClient) {
        // this.usersSubject = new BehaviorSubject<any>(null);
    }

    getAllUsers() : Observable<any> {
        return this.http.get<any>(
            `https://localhost:44382/api/users`);

        // return this.http.get<any>(`https://localhost:44382/api/users`)
        //     .pipe(map(response => {
        //         this.users = response;
        //         this.usersSubject.next(this.users);
        //         return response;
        //     }));
    }
}