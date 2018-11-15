import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs';
import { IProduct } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  public products = [];

  constructor(private _http : HttpClient) { }

  getProducts(): Observable<IProduct[]>{
    return this._http.get<IProduct[]>("http://localhost:8888/api/products");
  }




}
