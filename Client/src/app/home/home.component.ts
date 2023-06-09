import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  registrationStatus = false;
  users: any

  constructor() {}

  registrationToggle() {
    this.registrationStatus = !this.registrationStatus;
  }

  cancelRegistrationStatus(event: boolean) {
    this.registrationStatus = event;
  }
}
