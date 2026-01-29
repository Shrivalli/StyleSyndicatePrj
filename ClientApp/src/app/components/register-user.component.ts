import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, User } from '../services/api.service';

@Component({
  selector: 'app-register-user',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})
export class RegisterUserComponent implements OnInit {
  newUser: User = {
    id: 0,
    email: '',
    firstName: '',
    lastName: '',
    size: 'M',
    budget: 1000,
    dislikedColors: [],
    preferredBrands: [],
    avoidedMaterials: [],
    fitPreference: 'Regular',
    pastPurchaseIds: [],
    createdAt: new Date().toISOString()
  };

  dislikedColorInput: string = '';
  preferredBrandInput: string = '';
  avoidedMaterialInput: string = '';

  loading: boolean = false;
  success: boolean = false;
  error: string = '';

  sizes = ['XS', 'S', 'M', 'L', 'XL', 'XXL'];
  fitPreferences = ['Slim', 'Regular', 'Loose'];

  constructor(private apiService: ApiService) { }

  ngOnInit(): void { }

  addDislikedColor(): void {
    if (this.dislikedColorInput.trim()) {
      if (!this.newUser.dislikedColors.includes(this.dislikedColorInput.trim())) {
        this.newUser.dislikedColors = [...this.newUser.dislikedColors, this.dislikedColorInput.trim()];
        this.dislikedColorInput = '';
      }
    }
  }

  removeDislikedColor(color: string): void {
    this.newUser.dislikedColors = this.newUser.dislikedColors.filter(c => c !== color);
  }

  addPreferredBrand(): void {
    if (this.preferredBrandInput.trim()) {
      if (!this.newUser.preferredBrands.includes(this.preferredBrandInput.trim())) {
        this.newUser.preferredBrands = [...this.newUser.preferredBrands, this.preferredBrandInput.trim()];
        this.preferredBrandInput = '';
      }
    }
  }

  removePreferredBrand(brand: string): void {
    this.newUser.preferredBrands = this.newUser.preferredBrands.filter(b => b !== brand);
  }

  addAvoidedMaterial(): void {
    if (this.avoidedMaterialInput.trim()) {
      if (!this.newUser.avoidedMaterials.includes(this.avoidedMaterialInput.trim())) {
        this.newUser.avoidedMaterials = [...this.newUser.avoidedMaterials, this.avoidedMaterialInput.trim()];
        this.avoidedMaterialInput = '';
      }
    }
  }

  removeAvoidedMaterial(material: string): void {
    this.newUser.avoidedMaterials = this.newUser.avoidedMaterials.filter(m => m !== material);
  }

  registerUser(): void {
    // Validation
    if (!this.newUser.email.trim()) {
      this.error = 'Email is required';
      return;
    }

    if (!this.newUser.firstName.trim()) {
      this.error = 'First name is required';
      return;
    }

    if (!this.newUser.lastName.trim()) {
      this.error = 'Last name is required';
      return;
    }

    if (this.newUser.budget <= 0) {
      this.error = 'Budget must be greater than 0';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = false;

    this.apiService.createUser(this.newUser).subscribe({
      next: (response) => {
        this.success = true;
        this.loading = false;
        this.resetForm();
        // Auto-hide success message after 3 seconds
        setTimeout(() => {
          this.success = false;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Registration failed: ' + (err.error?.error || err.message);
        this.loading = false;
        console.error(err);
      }
    });
  }

  resetForm(): void {
    this.newUser = {
      id: 0,
      email: '',
      firstName: '',
      lastName: '',
      size: 'M',
      budget: 1000,
      dislikedColors: [],
      preferredBrands: [],
      avoidedMaterials: [],
      fitPreference: 'Regular',
      pastPurchaseIds: [],
      createdAt: new Date().toISOString()
    };
    this.dislikedColorInput = '';
    this.preferredBrandInput = '';
    this.avoidedMaterialInput = '';
  }
}
