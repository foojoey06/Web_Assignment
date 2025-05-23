﻿// Initiate GET request (AJAX-supported)
$(document).on('click', '[data-get]', e => {
    e.preventDefault();
    const url = e.target.dataset.get;
    location = url || location;
});

// Initiate POST request (AJAX-supported)
$(document).on('click', '[data-post]', e => {
    e.preventDefault();
    const url = e.target.dataset.post;
    const f = $('<form>').appendTo(document.body)[0];
    f.method = 'post';
    f.action = url || location;
    f.submit();
});

// Photo preview
$('.upload input').on('change', e => {
    const f = e.target.files[0];
    const img = $(e.target).siblings('img')[0];

    img.dataset.src ??= img.src;

    if (f && f.type.startsWith('image/')) {
        img.onload = e => URL.revokeObjectURL(img.src);
        img.src = URL.createObjectURL(f);
    }
    else {
        img.src = img.dataset.src;
        e.target.value = '';
    }

    // Trigger input validation
    $(e.target).valid();
});

//Multiple photo preview
$('.multiple input').on('change', e => {
    const f = e.target.files;
    const img = $(e.target).siblings('img')[0];

    img.dataset.src ??= img.src;
    clearInterval(img.cycleTimer);

    if (f.length && f[0].type.startsWith('image/')) {
        let index = 0;

        const cycle = () => {
            if (img.src.startsWith('blob:')) URL.revokeObjectURL(img.src);
            img.src = URL.createObjectURL(f[index]);
            index = (index + 1) % f.length;
        }

        cycle();
        img.cycleTimer = setInterval(cycle, 10000);
        
    }
    else {
        img.src = img.dataset.src;
        e.target.value = '';
    }

    // Trigger input validation
    $(e.target).valid();
});
