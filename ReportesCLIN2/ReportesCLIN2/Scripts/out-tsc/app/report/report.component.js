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