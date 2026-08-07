let clienteSeleccionado = false;

window.GolTime = (function () {

    function openModal(id) {
        const overlay = document.getElementById(id);
        if (!overlay) return;
        overlay.classList.add('open');
        document.body.style.overflow = 'hidden';
    }

    function closeModal(id) {
        const overlay = document.getElementById(id);
        if (!overlay) return;
        overlay.classList.remove('open');
        document.body.style.overflow = '';
    }

    document.addEventListener('click', function (e) {
        if (e.target.classList && e.target.classList.contains('modal-overlay')) {
            e.target.classList.remove('open');
            document.body.style.overflow = '';
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            document.querySelectorAll('.modal-overlay.open').forEach(function (overlay) {
                overlay.classList.remove('open');
            });
            document.body.style.overflow = '';
        }
    });

    document.addEventListener('click', function (e) {
        const pill = e.target.closest('.filter-bar .pill');
        if (!pill) return;
        pill.parentElement.querySelectorAll('.pill').forEach(p => p.classList.remove('active'));
        pill.classList.add('active');
    });

    document.addEventListener('click', function (e) {
        const editBtn = e.target.closest('.btn-icon.edit');
        if (!editBtn) return;

        const d = editBtn.dataset;
        setValue('editId', d.id);
        setValue('editClientId', d.clienteId);
        setValue('editUserId', d.userId);
        setValue('editFecha', d.fecha);
        setValue('editHoraInicio', d.horaInicio);
        setValue('editHoraFin', d.horaFin);
        setValue('editEstadoReserva', d.estadoReserva);
        setValue('editEstadoPago', d.estadoPago);

        openModal('modalEditar');
    });

    document.addEventListener('click', function (e) {
        const delBtn = e.target.closest('.btn-icon.delete');
        if (!delBtn) return;

        const d = delBtn.dataset;
        setValue('deleteId', d.id);
        setText('deleteClienteNombre', d.clienteNombre);
        setText('deleteFecha', d.fecha);
        setText('deleteHora', d.horaInicio + ' - ' + d.horaFin);
        setBadge('deleteEstadoPago', d.estadoPago, d.estadoPago === 'Pagado' ? 'badge-pagado' : 'badge-pendiente');
        setBadge('deleteEstadoReserva', d.estadoReserva, badgeClaseEstado(d.estadoReserva));

        openModal('modalEliminar');
    });

    function badgeClaseEstado(estado) {
        if (estado === 'Activa') return 'badge-activa';
        if (estado === 'Finalizada') return 'badge-finalizada';
        if (estado === 'Cancelada') return 'badge-cancelada';
        return 'badge-pendiente';
    }

    function setValue(id, value) {
        const el = document.getElementById(id);
        if (el) el.value = value ?? '';
    }

    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) el.textContent = value ?? '—';
    }

    function setBadge(id, text, cssClass) {
        const el = document.getElementById(id);
        if (!el) return;
        el.textContent = text ?? '—';
        el.className = 'badge ' + cssClass;
    }

    return { openModal, closeModal };

})();



function initClienteCombo() {

    const combo = document.getElementById('clienteCombo');
    if (!combo) return;

    const searchUrl = combo.dataset.searchUrl || '/Reservacion/BuscarClientesAjax';
    const search = document.getElementById('clienteSearch');
    const hidden = document.getElementById('clienteIdHidden');
    const list = document.getElementById('clienteList');

    let debounceTimer = null;

    function ocultarErrorCliente() {
        const err = document.getElementById('clienteError');
        if (err) err.style.display = 'none';
    }

    function render(items) {
        list.innerHTML = '';

        if (items.length === 0) {
            const empty = document.createElement('div');
            empty.className = 'client-combo-empty';
            empty.textContent = 'Sin coincidencias. Usa "+ Nuevo cliente".';
            list.appendChild(empty);
        } else {
            items.forEach(function (c) {
                const item = document.createElement('div');
                item.className = 'client-combo-item';
                item.textContent = c.nombre + ' — ' + c.numero;

                item.addEventListener('click', function () {
                    hidden.value = c.id;
                    search.value = c.nombre + ' — ' + c.numero;

                    list.innerHTML = '';
                    list.classList.remove('open');

                    ocultarErrorCliente();
                    search.blur();
                });

                list.appendChild(item);
            });
        }

        list.classList.add('open');
    }

    function buscarEnBD(term) {
        fetch(searchUrl + '?term=' + encodeURIComponent(term))
            .then(function (r) {
                if (!r.ok) throw new Error('Respuesta no válida');
                return r.json();
            })
            .then(function (items) {
                render(items);
            })
            .catch(function () {
                list.innerHTML = '';
                const err = document.createElement('div');
                err.className = 'client-combo-empty';
                err.textContent = 'No se pudo consultar la base de datos.';
                list.appendChild(err);
                list.classList.add('open');
            });
    }

    search.addEventListener('input', function () {

        if (clienteSeleccionado) {
            clienteSeleccionado = false;
            hidden.value = '';
            list.innerHTML = '';
            list.classList.remove('open');
            return;
        }

        hidden.value = '';

        const term = search.value.trim();

        if (term.length < 2) {
            list.innerHTML = '';
            list.classList.remove('open');
            return;
        }

        clearTimeout(debounceTimer);

        debounceTimer = setTimeout(function () {
            buscarEnBD(term);
        }, 250);
    });

    search.addEventListener('focus', function () {
        if (search.value.trim().length >= 2) {
            buscarEnBD(search.value.trim());
        }
    });

    document.addEventListener('click', function (e) {
        if (!combo.contains(e.target)) {
            list.classList.remove('open');
        }
    });

    const btnNuevo = document.getElementById('btnNuevoCliente');

    if (btnNuevo) {
        btnNuevo.addEventListener('click', function () {
            GolTime.openModal('modalNuevoCliente');
        });
    }

    const formNuevoCliente = document.getElementById('formNuevoCliente');

    if (formNuevoCliente) {
        formNuevoCliente.addEventListener('submit', function (e) {

            e.preventDefault();

            const formData = new FormData(formNuevoCliente);

            fetch(formNuevoCliente.action, {
                method: 'POST',
                body: formData
            })
                .then(function (r) {
                    return r.json();
                })
                .then(function (nuevo) {

                    if (!nuevo || !nuevo.id) return;

                    hidden.value = nuevo.id;
                    search.value = nuevo.nombre + ' — ' + nuevo.numero;

                    ocultarErrorCliente();

                    formNuevoCliente.reset();
                    GolTime.closeModal('modalNuevoCliente');
                })
                .catch(function () {
                    alert('No se pudo guardar el cliente. Intenta de nuevo.');
                });
        });
    }
}

initClienteCombo();