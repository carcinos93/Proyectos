import { Component, OnInit } from '@angular/core';
import { URL } from 'url';

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

    cardOnClick = function (event, data: any) {
        let reporte: string  = data.nombre;
        window.open("/Home/ReportViewer?reporte=" + reporte , "_blank");
    }

    ngOnInit() {
        var data = [
            { nombre: "MATRIZ_RIESGO_CLIENTE", descripcion: "Matriz de riesgo cliente", tipo: "report" },
            { nombre: "MATRIZ_RIESGO_PROVEEDOR", descripcion: "Matriz de riesgo proveedor", tipo: "report" }

        ];
        var grid = this.matrixList(data, 4);
        this.datos = grid;

  }

}
