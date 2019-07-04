import { Component, OnInit } from '@angular/core';
import { Users } from 'src/app/models/users';
import { UserService } from 'src/app/services/users.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

    public users: any = null;
    public displayedColumns: string[] = ['firstName', 'lastName', 'username',
    'email','userRole'];

    constructor(private userService: UserService) {
        this.getAllUsers();
      }

    ngOnInit() {
    }

    getAllUsers() {
        // this.users = []
        this.userService.getAllUsers().subscribe(u => {
            this.users = u;
            console.log(u);
        });
    }
}
