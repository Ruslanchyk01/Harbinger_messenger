import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.scss']
})
export class TestErrorsComponent {
  basicUrl = environment.apiUrl;
  validationError: string[] = [];

  constructor(private http: HttpClient) {}

  get400() {
    this.http.get(this.basicUrl + 'bug/badrequest').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get404() {
    this.http.get(this.basicUrl + 'bug/notfound').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get500() {
    this.http.get(this.basicUrl + 'bug/servererror').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get401() {
    this.http.get(this.basicUrl + 'bug/auth').subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get400Validation() {
    this.http.post(this.basicUrl + 'account/registration', {}).subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
      this.validationError = error;
    })
  }
}
