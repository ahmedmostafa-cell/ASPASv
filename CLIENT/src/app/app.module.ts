import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TextBoxModule, InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { PopupModule } from '@progress/kendo-angular-popup';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LabelModule } from '@progress/kendo-angular-label';
import { ToastrModule } from 'ngx-toastr';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    GridModule,
    AppRoutingModule,
    HttpClientModule,
    TextBoxModule,
    ButtonsModule,
    PopupModule,
    DialogsModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LabelModule,
    InputsModule,
    ToastrModule.forRoot({
      timeOut: 3000, // Display for 3 seconds
      positionClass: 'toast-bottom-right', // Position (e.g., bottom-right, top-left)
      preventDuplicates: true, // Prevent showing duplicate toasts
      progressBar: true, // Show progress bar
    }),
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
