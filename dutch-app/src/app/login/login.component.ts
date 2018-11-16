import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: []
})
export class LoginComponent implements OnInit {

  public errorMessage : string;
  public creds: any = {
    username:"",
    password:""
  };

  constructor(private _dataService: DataService, private _router: Router) { }

  ngOnInit() {
  }

  onLogin(){
    this._dataService.login(this.creds)
    .subscribe(success => {
      if(success){
        this._router.navigate(['buy']);
      }
    },
    error => this.errorMessage = "Failed to login")
  }

}
