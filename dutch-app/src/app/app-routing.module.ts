import { NgModule } from '@angular/core';
import { RouterModule, Routes }  from '@angular/router';
import { ShopComponent } from './shop/shop.component';
import { BuyComponent } from './shop/buy.component';
import { PageNotFoundComponent } from './shop/page-not-found.component';
import { LoginComponent } from './login/login.component';

const APP_ROUTES: Routes = [
  { path: 'shop', component: ShopComponent },
  { path: 'buy',  component: BuyComponent },
  { path: 'login',  component: LoginComponent },
  { path: '',   redirectTo: '/shop', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(
      APP_ROUTES
    )
  ],
  exports: [
    RouterModule
  ]
})

export class AppRoutingModule { }
