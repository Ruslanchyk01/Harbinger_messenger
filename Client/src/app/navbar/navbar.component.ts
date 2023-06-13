import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  model: any = {}

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {}

  logIn() {
    this.accountService.logIn(this.model).subscribe(response => {
      this.router.navigateByUrl('/users');
    })
  }

  logOut() {
    this.accountService.logOut();
    this.router.navigateByUrl('/');
  }
}
