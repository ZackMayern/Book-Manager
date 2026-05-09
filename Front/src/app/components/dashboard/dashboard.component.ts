import { Component, OnInit } from '@angular/core';
import { MonitoringComponent } from "./monitoring/monitoring.component";
import { ViewbookComponent } from "./viewbook/viewbook.component";
import { RequestsComponent } from '../requests/requests.component';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MonitoringComponent, ViewbookComponent, RequestsComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  isAdmin: boolean = false;

  constructor(private readonly authService: AuthService) {}

  public ngOnInit() {
    this.isAdmin = this.authService.hasRole('Admin');
  }

  public showAdminContent(): boolean {
    return this.isAdmin;
  }
}
