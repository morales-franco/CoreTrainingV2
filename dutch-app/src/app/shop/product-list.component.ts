import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { IProduct } from '../models/product';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {

  public products :IProduct[];

  constructor(private _dataService : DataService,
    private _router: Router) { }

  ngOnInit() {
    this.getProducts();
  }

  getProducts(): void {
    this._dataService.getProducts()
    .subscribe(products => this.products = products);
  }

  onCheckout(){
    if(this._dataService.loginRequired){
      this._router.navigate(['login']);
    }else{
      this._router.navigate(['buy']);
    }
  }


}

