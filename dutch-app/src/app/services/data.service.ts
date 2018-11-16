import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs';
import { IProduct } from '../models/product';
import {  map } from 'rxjs/operators';
import { Buy } from '../models/buy';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private token: string = "";
  private tokenExpiration : Date;

  constructor(private _http : HttpClient) { }

  public getProducts(): Observable<IProduct[]>{
    return this._http.get<IProduct[]>("http://localhost:8888/api/products");
  }

  public get loginRequired(): boolean{
    return this.token.length == 0  || this.tokenExpiration > new Date();
  }

  public login(creds : any) : Observable<boolean>{
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json'
      })
    };

    return this._http.post<boolean>("http://localhost:8888/account/createtoken", creds, httpOptions)
               .pipe(map((data : any) => {
                  this.token = data.token;
                  this.tokenExpiration = data.expiration;
                  return true;
                }));

  }

  public buy(data : Buy): Observable<boolean>{
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/json',
        'Authorization' : 'Bearer ' + this.token
      })
    };
    return this._http.post<boolean>("http://localhost:8888/api/orders/buy", data, httpOptions);


  }


}
