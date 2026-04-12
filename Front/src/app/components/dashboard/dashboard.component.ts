import { Component, OnInit } from '@angular/core';
import { MonitoringComponent } from "./monitoring/monitoring.component";
import { ViewbookComponent } from "./viewbook/viewbook.component";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MonitoringComponent, ViewbookComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  isAdmin: boolean = false;

  public ngOnInit() {
    const userRole = localStorage.getItem('userRole');
    this.isAdmin = userRole === 'Admin';
  }

  public showAdminContent(): boolean {
    return this.isAdmin;
  }
}
