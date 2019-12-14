var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Component } from '@angular/core';
let ReportComponent = class ReportComponent {
    constructor() {
        this.matrixList = function (data, n) {
            var grid = [], i = 0, x = data.length, col, row = -1;
            for (var i = 0; i < x; i++) {
                col = i % n;
                if (col === 0) {
                    grid[++row] = [];
                }
                grid[row][col] = data[i];
            }
            return grid;
        };
        this.cardOnClick = function (event, data) {
            let reporte = data.nombre;
            window.open("/Home/ReportViewer?reporte=" + reporte, "_blank");
        };
    }
    ngOnInit() {
        var data = [
            { nombre: "MATRIZ_RIESGO_CLIENTE", descripcion: "Matriz de riesgo cliente", tipo: "report" },
            { nombre: "MATRIZ_RIESGO_PROVEEDOR", descripcion: "Matriz de riesgo proveedor", tipo: "report" }
        ];
        var grid = this.matrixList(data, 4);
        this.datos = grid;
    }
};
ReportComponent = __decorate([
    Component({
        selector: 'app-report',
        templateUrl: './report.component.html',
        styles: []
    })
], ReportComponent);
export { ReportComponent };
//# sourceMappingURL=report.component.js.map