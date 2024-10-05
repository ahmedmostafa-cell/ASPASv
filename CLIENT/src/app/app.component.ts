import { HttpClient } from '@angular/common/http';
import { Component , OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr'; // Import ToastrService
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  public registerForm: FormGroup = new FormGroup({
    id: new FormControl(),
    firstName: new FormControl("",Validators.required),
    lastName: new FormControl("" , Validators.required),
    emailAddress: new FormControl("", [Validators.email , Validators.required] ),
    phoneNumber: new FormControl("" , [Validators.required , Validators.pattern(/^\d+$/)]),
  });
  title = 'CLIENT';
  public isDialogVisible: boolean = false;
  clients : any;
  public dialogOpened = false;
  public isEditing = false;  

  constructor(private http : HttpClient , private toastr :ToastrService){};

  ngOnInit(): void {
   
    this.GetClients();
    
  }

  public GetClients() 
  {
    this.http.get('http://localhost:5000/api/Client').subscribe({
      next: response =>  
        {
          this.clients = response;
          console.log(this.clients);
        },
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    })
  }

  public Edit(dataitem: any): void {
    this.open();
    this.isEditing = true;  // Set edit mode to true
    this.registerForm.patchValue({
      id : dataitem.id,
      firstName: dataitem.firstName,
      lastName: dataitem.lastName,
      emailAddress: dataitem.emailAddress,
      phoneNumber: dataitem.phoneNumber,
    });
  }
  
  public Delete(dataitem: any): void {
    const clientId = dataitem.id; 
    const url = `http://localhost:5000/api/Client/${clientId}`;
  
    this.http.delete(url).subscribe(
      response => {
        console.log("Delete successful", response);
        this.toastr.success("Client deleted successfully.", "Success");
        this.GetClients();
        this.clearForm();
        
      },
      error => {
        console.error("Delete failed", error);
        this.toastr.error("Error deleting client: " + error.message, "Error");
        this.clearForm();
      }
    );
  }

  public Add(): void {
    this.open();
  }

  public close(): void {
    this.dialogOpened = false;
  }

  public open(): void {
    this.dialogOpened = true;
  }

  public submitForm(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.registerForm.markAllAsTouched();
    console.log(this.registerForm);
    if(!this.isEditing) 
      {
        this.registerForm.value['id']=1;
        console.log(this.registerForm.value);
        this.http.post('http://localhost:5000/api/Client', this.registerForm.value)
        .subscribe(
          response => {
            console.log('Server Response:', response);
            this.GetClients();
            this.toastr.info("The Client has been registered" , "Client added");
            this.dialogOpened = false;
            this.clearForm();
          },
          error => {
            console.error('Error occurred:', error);
            this.toastr.error("Email already has been registered before" , "Client not added");
            this.clearForm();
          }
        );
        this.dialogOpened = false;
    }
    else 
    {
      console.log(this.registerForm.value)
      this.http.put(`http://localhost:5000/api/Client/${this.registerForm.value['id']}`, this.registerForm.value)
      .subscribe(
        response => {
          console.log('Server Response:', response);
          this.GetClients();
          this.toastr.info("The Client has been updated", "Client updated");
          this.dialogOpened = false;
          this.clearForm();
          this.isEditing = false;
        },
        error => {
          console.error('Error occurred:', error);
          this.toastr.error("Error updating client", "Client not updated");
          this.clearForm();
        }
      );
    }
 
  }

  public clearForm(): void {
    this.registerForm.reset();
  }
  
}