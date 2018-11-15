import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { IProduct } from '../models/product';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {

  public products :IProduct[];

  constructor(private _dataService : DataService) { }

  ngOnInit() {
    this.getProducts();
  }

  getProducts(): void {
    this._dataService.getProducts()
    .subscribe(products => this.products = products);
  }

}

