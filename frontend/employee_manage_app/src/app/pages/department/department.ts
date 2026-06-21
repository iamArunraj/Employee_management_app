import { Component, inject, OnInit, signal } from '@angular/core';
import { DepartmentModel } from '../../models/Department.model';
import { FormsModule } from '@angular/forms';
import { Master } from '../../services/master';

@Component({
  selector: 'app-department',
  imports: [FormsModule],
  templateUrl: './department.html',
  styleUrl: './department.css',
})
export class Department implements OnInit {
  newDepObj: DepartmentModel = new DepartmentModel();
  masterService = inject(Master)
  deptList = signal<DepartmentModel[]>([]);

  ngOnInit(): void {
    this.getAllDepartments();
  }

  onSaveDept() {
    this.masterService.saveDept(this.newDepObj).subscribe({
      next: (result: any) => {
        debugger;
        alert('Department Added Successfully');
        this.getAllDepartments();
      },
      error: (error) => {
        debugger;
        alert(error.error)
      }
    })
  }

  onUpdateDept() {
    this.masterService.updateDept(this.newDepObj).subscribe({
      next: (result: any) => {
        debugger;
        alert('Department updated Successfully');
        this.getAllDepartments();
      },
      error: (error) => {
        debugger;
        alert(error.error)
      }
    })
  }

  onEdit(data: DepartmentModel) {
    const strData = JSON.stringify(data);
    const parseData = JSON.parse(strData);
    this.newDepObj = parseData;
  }

  onDelete(id: number) {
    const isDelete = confirm('Are you sure you want to delete this department?');
    if (isDelete) {
      this.masterService.deleteDeptbyId(id).subscribe({
        next: (result: any) => {
          alert('Department deleted Successfully');
          this.getAllDepartments();
        },
        error: (error) => {
          alert(error.error)
        }
      })
    }
  }

  onReset() {
    this.newDepObj = new DepartmentModel();
  }

  getAllDepartments() {
    this.masterService.getAllDept().subscribe({
      next: (result: any) => {
        this.deptList.set(result);
      }
    })
  }
}
