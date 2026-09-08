import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

type VehicleType = 'Sedan' | 'SUV' | 'Van' | 'Pickup' | 'Premium';

interface Vehicle {
  id: number;
  plate: string;
  brand: string;
  model: string;
  type: string;
  dailyRate: number;
  isActive: boolean;
}

interface AvailabilityResponse {
  vehicleType: string;
  startDate: string;
  endDate: string;
  availableVehicles: Vehicle[];
}

interface BookingHistoryItem {
  id: number;
  clientId: number;
  vehicleId: number;
  bookingReference: string;
  startDate: string;
  endDate: string;
  totalAmount: number;
  status: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'RentaFácil';
  vehicleTypes: VehicleType[] = ['Sedan', 'SUV', 'Van', 'Pickup', 'Premium'];
  availability = {
    vehicleType: 'SUV' as VehicleType,
    startDate: '2026-09-10',
    endDate: '2026-09-15'
  };

  availableVehicles: Vehicle[] = [];
  statusMessage = 'Listo para consultar disponibilidad.';
  isLoading = false;

  clientForm = {
    documentNumber: '1012345678',
    name: 'Ana',
    lastName: 'García',
    email: 'ana@rentafacil.com'
  };

  bookingForm = {
    clientId: 1,
    vehicleId: 0,
    startDate: '2026-09-10',
    endDate: '2026-09-15',
    dailyRate: 0
  };

  historyFiltro = 1;
  bookingHistory: BookingHistoryItem[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.searchAvailability();
    this.loadHistory();
  }

  searchAvailability(): void {
    this.isLoading = true;
    this.statusMessage = 'Consultando vehículos disponibles...';

    const query = new URLSearchParams({
      vehicleType: this.availability.vehicleType,
      startDate: this.availability.startDate,
      endDate: this.availability.endDate
    });

    this.http.get<AvailabilityResponse>(`http://localhost:5201/api/vehicles/availability?${query.toString()}`)
      .subscribe({
        next: (response) => {
          this.availableVehicles = response.availableVehicles ?? [];
          this.statusMessage = `Se encontraron ${this.availableVehicles.length} vehículos disponibles.`;
          this.isLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          this.statusMessage = `No se pudo consultar disponibilidad: ${error.message}`;
          this.availableVehicles = [];
          this.isLoading = false;
        }
      });
  }

  selectVehicle(vehicle: Vehicle): void {
    this.bookingForm.vehicleId = vehicle.id;
    this.bookingForm.dailyRate = vehicle.dailyRate;
    this.bookingForm.startDate = this.availability.startDate;
    this.bookingForm.endDate = this.availability.endDate;
    this.statusMessage = `Vehículo ${vehicle.brand} ${vehicle.model} seleccionado.`;
  }

  createClient(): void {
    this.http.post('http://localhost:5202/api/bookings/clients', this.clientForm)
      .subscribe({
        next: (response: any) => {
          this.bookingForm.clientId = response.id;
          this.historyFiltro = response.id;
          this.statusMessage = `Cliente creado: ${response.name} ${response.lastName}`;
          this.loadHistory();
        },
        error: (error: HttpErrorResponse) => {
          this.statusMessage = `Error al crear cliente: ${error.message}`;
        }
      });
  }

  createBooking(): void {
    const payload = {
      clientId: this.bookingForm.clientId,
      vehicleId: this.bookingForm.vehicleId,
      startDate: this.bookingForm.startDate,
      endDate: this.bookingForm.endDate,
      dailyRate: this.bookingForm.dailyRate
    };

    this.http.post('http://localhost:5202/api/bookings', payload)
      .subscribe({
        next: (response: any) => {
          this.statusMessage = `Reserva creada correctamente: ${response.bookingReference}`;
          this.searchAvailability();
          this.loadHistory();
        },
        error: (error: HttpErrorResponse) => {
          this.statusMessage = `No se pudo crear la reserva: ${error.message}`;
        }
      });
  }

  loadHistory(): void {
    this.http.get<BookingHistoryItem[]>(`http://localhost:5202/api/bookings/client/${this.historyFiltro}`)
      .subscribe({
        next: (response) => {
          this.bookingHistory = response ?? [];
        },
        error: () => {
          this.bookingHistory = [];
        }
      });
  }
}
