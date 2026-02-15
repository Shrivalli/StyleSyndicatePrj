import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  size: string;
  budget: number;
  dislikedColors: string[];
  preferredBrands: string[];
  avoidedMaterials: string[];
  fitPreference: string;
  pastPurchaseIds: number[];
  createdAt: string;
}

export interface Product {
  id: number;
  name: string;
  description: string;
  category: string;
  material: string;
  color: string;
  price: number;
  availableSizes: string[];
  brand: string;
  tags: string[];
  inStock: boolean;
  imageUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'https://localhost:7208/api';

  constructor(private http: HttpClient) { }

  // User endpoints
  getUser(userId: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/users/${userId}`);
  }

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/users`, user);
  }

  updateUser(user: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/users`, user);
  }

  // Product endpoints
  getProduct(productId: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/products/${productId}`);
  }

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/products`);
  }

  searchProducts(criteria: any): Observable<Product[]> {
    return this.http.post<Product[]>(`${this.apiUrl}/products/search`, criteria);
  }

  // Style consultation endpoint
  curateOutfit(userId: number, request: string): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/stylesyndicate/curate-outfit?userId=${userId}`,
      { userRequest: request }
    );
  }
}
