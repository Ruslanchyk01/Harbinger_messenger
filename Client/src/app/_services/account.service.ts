import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  basicUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) { }
  
  registration(model: any) {
    return this.http.post(this.basicUrl + 'account/registration', model).pipe(
      map((user: User) => {
        if(user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  logIn(model: any) {
    return this.http.post(this.basicUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if(user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  logOut() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();

  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
}
