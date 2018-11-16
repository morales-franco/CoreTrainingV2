import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { Buy } from '../models/buy';

@Component({
  selector: 'app-buy',
  templateUrl: './buy.component.html',
  styleUrls: ['./buy.component.css']
})
export class BuyComponent implements OnInit {

  constructor(private _dataService : DataService) { }

  ngOnInit() {
  }

  buy(){
    let data: Buy = {
        amount : 90.99,
        description : "Product"
    };

    this._dataService.buy(data)
    .subscribe(result => {
      if(result){
        alert("Compra Exitosa!");
      }else{
        alert("Fallo la compra");
      }
    })


  }

}
