import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, User } from '../services/api.service';

@Component({
  selector: 'app-style-request',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './style-request.component.html',
  styleUrls: ['./style-request.component.css']
})
export class StyleRequestComponent implements OnInit {
  users: User[] = [];
  selectedUserId: number = 1;
  requestText: string = 'I have a wedding in Tuscany next month, help me look sharp';
  loading: boolean = false;
  workflowResponse: any = null;
  error: string = '';

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.apiService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (err) => {
        this.error = 'Failed to load users: ' + err.message;
        console.error(err);
      }
    });
  }

  curateOutfit(): void {
    if (!this.requestText.trim()) {
      this.error = 'Please enter a style request';
      return;
    }

    this.loading = true;
    this.error = '';
    this.workflowResponse = null;

    this.apiService.curateOutfit(this.selectedUserId, this.requestText).subscribe({
      next: (response) => {
        this.workflowResponse = response;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error: ' + (err.error?.error || err.message);
        this.loading = false;
        console.error(err);
      }
    });
  }

  getAgentColor(agentName: string): string {
    const colors: { [key: string]: string } = {
      'The Concierge': '#1976d2',
      'The Historian': '#388e3c',
      'The Trend Analyst': '#f57c00',
      'The Inventory Scout': '#c2185b',
      'The Critic': '#7b1fa2'
    };
    return colors[agentName] || '#999';
  }
}
