toastr.options = {
    "positionClass": "toast-bottom-left"
};

oTable = $('#Tab').DataTable(
    {
        "language": {
            "lengthMenu": "Mostrando _MENU_ Entradas por pagina",
            "zeroRecords": "Lo sentimos, no se encuentran concidencias",
            "info": "Mostrando Página _PAGE_ de _PAGES_ (Registros Totales _TOTAL_)",
            "infoEmpty": "No hay entradas Disponibles",
            "infoFiltered": "(filtrar por _MAX_ total entradas)",
            "search": "Buscar",
            "paginate": {
                "previous": "Anterior",
                "next": "Siguiente"
            }
        }
    });

pTable = $('#Tabp').DataTable(
    {
        columnDefs: [
            {
                targets: [0, 1, 2],
                className: 'mdl-data-table__cell--non-numeric'
            }
        ],
        "language": {
            "lengthMenu": "Mostrando _MENU_ Entradas por Proforma",
            "zeroRecords": "Sin Servicios agregados",
            "info": "Mostrando Detalle _PAGE_ de _PAGES_",
            "infoEmpty": "No hay entradas Disponibles",
            "infoFiltered": "(filtrar por _MAX_ total entradas)"
        }
    });


$('#myInputTextField').keyup(function () {
    oTable.search($(this).val()).draw();
})

$('#telor').keyup(function (e) {

    if (e.keyCode >= 48 && e.keyCode <= 57) {
        var v = $(this).val();
        lv = v.length;
    } else {
        e.co
    }


})

Math.moda = function (array) {
    if (array !== undefined && array !== null) {

        //iniciamos las variables necesarias en todo el codigo
        var moda, moda2;
        var contador = 0, contador2 = 0;
        //Recorremos la array
        for (var x = 0; x < array.length; x++) {
            //Miramos que el numero cogido no sea el de la moda
            if (array[x] != moda) {
                var contadorReinicia = 0;
                //Recorremos la array para encontrar concordancias on el numero sacado de la array de X
                for (var i = 0; i < array.length; i++) {
                    //cunado el numero sea igual al de la array de x le añadimos 1 al contador
                    if (array[i] == array[x]) contadorReinicia++;

                }
                //si el contador que se reinicia nos da mas alto que el contador general añadimos el numero a la variable moda y cambiamos el contador general por el que reinicia
                if (contadorReinicia > contador) {
                    contador = contadorReinicia;
                    moda = array[x];

                }

            }

        }
        //Miramos que no hayan 2 con la misma cantidad
        for (var x = 0; x < array.length; x++) {
            //Miramos que el numero cogido no sea el de la moda
            if (array[x] != moda && array[x] != moda2) {
                var contadorReinicia = 0;
                //Recorremos la array para encontrar concordancias on el numero sacado de la array de X
                for (var i = 0; i < array.length; i++) {
                    //cunado el numero sea igual al de la array de x le añadimos 1 al contador
                    if (array[i] == array[x]) contadorReinicia++;

                }
                //si el contador que se reinicia nos da mas alto que el contador general añadimos el numero a la variable moda y cambiamos el contador general por el que reinicia
                if (contadorReinicia > contador2) {
                    contador2 = contadorReinicia;
                    moda2 = array[x];

                }
                //Si tenemos 2 de la misma cantidad retornamos -1
                if (contador2 == contador) return moda;

            }

        }
        //Retornamos la moda!!!
        return moda;
    }
    return null;
}

function NoLetras(cadena) {
    if (isNaN(cadena) && cadena.length > 0) {
        cadena = cadena.substring(0, cadena.length - 1);

    }
    return cadena;

}


function Telefono(cadena) {
    cadena = NoLetras(cadena);
    var punto = cadena.substring(cadena.length - 1, cadena.length);
    if (cadena.length > 8 || cadena.indexOf('.') != -1) {
        cadena = cadena.substring(0, cadena.length - 1);
    }

    return cadena;
}

function Cedula(cadena) {
    var bool = true

    if (!(/^\d{1}$/.test(cadena)) && cadena.length == 1) {
        bool = false;
    }
    if (!(/^\d{2}$/.test(cadena)) && cadena.length == 2) {
        bool = false;
    }
    if (!(/^\d{3}$/.test(cadena)) && cadena.length == 3) {
        bool = false;
    }
    if (!(/^\d{4}$/.test(cadena)) && cadena.length == 4) {
        bool = false;
    }
    if (!(/^\d{5}$/.test(cadena)) && cadena.length == 5) {
        bool = false;
    }
    if (!(/^\d{6}$/.test(cadena)) && cadena.length == 6) {
        bool = false;
    }
    if (!(/^\d{7}$/.test(cadena)) && cadena.length == 7) {
        bool = false;
    }
    if (!(/^\d{8}$/.test(cadena)) && cadena.length == 8) {
        bool = false;
    }
    if (!(/^\d{9}$/.test(cadena)) && cadena.length == 9) {
        bool = false;
    }
    if (!(/^\d{10}$/.test(cadena)) && cadena.length == 10) {
        bool = false;
    }
    if (!(/^\d{11}$/.test(cadena)) && cadena.length == 11) {
        bool = false;
    }
    if (!(/^\d{12}$/.test(cadena)) && cadena.length == 12) {
        bool = false;
    }

    if (!(/^\d{13}$/.test(cadena)) && cadena.length == 13) {
        bool = false;
    }

    if (!(/^\d{13}[A-Z]{1}$/.test(cadena)) && cadena.length == 14) {
        bool = false;
    }

 

    if (cadena.length > 14) {
        bool = false;
    }

    if (bool == false) {
        cadena = cadena.substring(0, cadena.length - 1);
    }
    return cadena;

}

function EsRuc(cadena) {
    bool = true;
    if (!(/^[A-Z]$/.test(cadena)) && cadena.length == 1) {
        bool = false;
    }
    if (!(/^[A-Z]\d{1}$/.test(cadena)) && cadena.length == 2) {
        bool = false;
    }
    if (!(/^[A-Z]\d{2}$/.test(cadena)) && cadena.length == 3) {
        bool = false;
    }
    if (!(/^[A-Z]\d{3}$/.test(cadena)) && cadena.length == 4) {
        bool = false;
    }
    if (!(/^[A-Z]\d{4}$/.test(cadena)) && cadena.length == 5) {
        bool = false;
    }
    if (!(/^[A-Z]\d{5}$/.test(cadena)) && cadena.length == 6) {
        bool = false;
    }
    if (!(/^[A-Z]\d{6}$/.test(cadena)) && cadena.length == 7) {
        bool = false;
    }
    if (!(/^[A-Z]\d{7}$/.test(cadena)) && cadena.length == 8) {
        bool = false;
    }
    if (!(/^[A-Z]\d{8}$/.test(cadena)) && cadena.length == 9) {
        bool = false;
    }
    if (!(/^[A-Z]\d{9}$/.test(cadena)) && cadena.length == 10) {
        bool = false;
    }
    if (!(/^[A-Z]\d{10}$/.test(cadena)) && cadena.length == 11) {
        bool = false;
    }
    if (!(/^[A-Z]\d{11}$/.test(cadena)) && cadena.length == 12) {
        bool = false;
    }

    if (!(/^[A-Z]\d{12}$/.test(cadena)) && cadena.length == 13) {
        bool = false;
    }
    if (!(/^[A-Z]\d{13}$/.test(cadena)) && cadena.length == 14) {
        bool = false;
    }
    if (cadena.length > 14) {
        bool = false;
    }
    if (bool == false) {
        cadena = cadena.substring(0, cadena.length - 1);
    }
    return cadena;
}




function Unidades(num) {

    switch (num) {
        case 1: return "UN";
        case 2: return "DOS";
        case 3: return "TRES";
        case 4: return "CUATRO";
        case 5: return "CINCO";
        case 6: return "SEIS";
        case 7: return "SIETE";
        case 8: return "OCHO";
        case 9: return "NUEVE";
    }

    return "";
}

function Decenas(num) {

    decena = Math.floor(num / 10);
    unidad = num - (decena * 10);

    switch (decena) {
        case 1:
            switch (unidad) {
                case 0: return "DIEZ";
                case 1: return "ONCE";
                case 2: return "DOCE";
                case 3: return "TRECE";
                case 4: return "CATORCE";
                case 5: return "QUINCE";
                default: return "DIECI" + Unidades(unidad);
            }
        case 2:
            switch (unidad) {
                case 0: return "VEINTE";
                default: return "VEINTI" + Unidades(unidad);
            }
        case 3: return DecenasY("TREINTA", unidad);
        case 4: return DecenasY("CUARENTA", unidad);
        case 5: return DecenasY("CINCUENTA", unidad);
        case 6: return DecenasY("SESENTA", unidad);
        case 7: return DecenasY("SETENTA", unidad);
        case 8: return DecenasY("OCHENTA", unidad);
        case 9: return DecenasY("NOVENTA", unidad);
        case 0: return Unidades(unidad);
    }
}//Unidades()

function DecenasY(strSin, numUnidades) {
    if (numUnidades > 0)
        return strSin + " Y " + Unidades(numUnidades)

    return strSin;
}//DecenasY()

function Centenas(num) {

    centenas = Math.floor(num / 100);
    decenas = num - (centenas * 100);

    switch (centenas) {
        case 1:
            if (decenas > 0)
                return "CIENTO " + Decenas(decenas);
            return "CIEN";
        case 2: return "DOSCIENTOS " + Decenas(decenas);
        case 3: return "TRESCIENTOS " + Decenas(decenas);
        case 4: return "CUATROCIENTOS " + Decenas(decenas);
        case 5: return "QUINIENTOS " + Decenas(decenas);
        case 6: return "SEISCIENTOS " + Decenas(decenas);
        case 7: return "SETECIENTOS " + Decenas(decenas);
        case 8: return "OCHOCIENTOS " + Decenas(decenas);
        case 9: return "NOVECIENTOS " + Decenas(decenas);
    }

    return Decenas(decenas);
}//Centenas()

function Seccion(num, divisor, strSingular, strPlural) {
    cientos = Math.floor(num / divisor)
    resto = num - (cientos * divisor)

    letras = "";

    if (cientos > 0)
        if (cientos > 1)
            letras = Centenas(cientos) + " " + strPlural;
        else
            letras = strSingular;

    if (resto > 0)
        letras += "";

    return letras;
}//Seccion()

function Miles(num) {
    divisor = 1000;
    cientos = Math.floor(num / divisor)
    resto = num - (cientos * divisor)

    strMiles = Seccion(num, divisor, "UN MIL", "MIL");
    strCentenas = Centenas(resto);

    if (strMiles == "")
        return strCentenas;

    return strMiles + " " + strCentenas;

    //return Seccion(num, divisor, "UN MIL", "MIL") + " " + Centenas(resto);
}//Miles()

function Millones(num) {
    divisor = 1000000;
    cientos = Math.floor(num / divisor)
    resto = num - (cientos * divisor)

    strMillones = Seccion(num, divisor, "UN MILLON", "MILLONES");
    strMiles = Miles(resto);

    if (strMillones == "")
        return strMiles;

    return strMillones + " " + strMiles;

    //return Seccion(num, divisor, "UN MILLON", "MILLONES") + " " + Miles(resto);
}//Millones()

function NumeroALetras(num, moneda) {
    var data = {
        numero: num,
        enteros: Math.floor(num),
        centavos: (((Math.round(num * 100)) - (Math.floor(num) * 100))),
        letrasCentavos: "",
        letrasMonedaPluralc: "CORDOBAS",
        letrasMonedaSingularc: "CORDOBA",
        letrasMonedaPlural: "DOLARES",
        letrasMonedaSingular: "DOLAR"

    };




    if (data.centavos > 0)
        data.letrasCentavos = "CON " + data.centavos + "/100";

    if (data.enteros == 0)
        if (moneda == 1) {
            return "CERO " + data.letrasMonedaPlural + " " + data.letrasCentavos;
        } else {
            return "CERO " + data.letrasMonedaPluralc + " " + data.letrasCentavos;
        }

    if (data.enteros == 1)
        if (moneda == 1) {
            return Millones(data.enteros) + " " + data.letrasMonedaSingular + " " + data.letrasCentavos;
        } else {
            return Millones(data.enteros) + " " + data.letrasMonedaSingularc + " " + data.letrasCentavos;
        }
    else
        if (moneda == 1) {
            return Millones(data.enteros) + " " + data.letrasMonedaPlural + " " + data.letrasCentavos;
        } else {
            return Millones(data.enteros) + " " + data.letrasMonedaPluralc + " " + data.letrasCentavos;
        }
}//NumeroALetras()



