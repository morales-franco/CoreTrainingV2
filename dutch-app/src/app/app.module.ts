import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule }    from '@angular/common/http';

import { AppComponent } from './app.component';
import { ProductListComponent } from './shop/product-list.component';
import { CartComponent } from './shop/cart.component'

@NgModule({
  declarations: [
    AppComponent,
    ProductListComponent,
    CartComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
