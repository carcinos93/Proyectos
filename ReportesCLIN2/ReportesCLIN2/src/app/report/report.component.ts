import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styles: []
})
export class ReportComponent implements OnInit {
    datos: any[];
    constructor() {
        
    }
    matrixList = function (data, n) {
        var grid = [], i = 0, x = data.length, col, row = -1;
        for (var i = 0; i < x; i++) {
            col = i % n;
            if (col === 0) {
                grid[++row] = [];
            }
            grid[row][col] = data[i];
        }
        return grid;
    }
    ngOnInit() {
        var data = [
            { nombre: "reporte01", descripcion: "Lorem ipsum dolor sit amet, consectetur", tipo: "report 1" },
            { nombre: "reporte02", descripcion: "or incididunt ut labore et dolore magna ", tipo: "report 2" },
            { nombre: "reporte03", descripcion: "Duis aute irure dolor in reprehenderit ", tipo: "report 3" },
            { nombre: "reporte04", descripcion: "e it is pleasure, but because those who do not know how to pursue ", tipo: "report 4" },
            { nombre: "reporte05", descripcion: "se, except to obtain some advantage fr", tipo: "report 5" },
        ];
        var grid = this.matrixList(data, 4);
        this.datos = grid;

  }

}
