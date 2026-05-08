// ── Cart ──
function addToCart(productId, btn) {
    const $btn = $(btn);
    const orig = $btn.html();
    $btn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i> Ekleniyor...');
    $.post('/Cart/Add', { productId, quantity: 1 }, function (res) {
        if (res.success) {
            $btn.html('<i class="bi bi-check-lg"></i> Eklendi!');
            const b = $('#cartBadge');
            b.text(res.cartCount).show();
            toastr.success(res.message || 'Ürün sepete eklendi');
            setTimeout(() => $btn.html(orig).prop('disabled', false), 1800);
        } else {
            toastr.error(res.message || 'Bir hata oluştu');
            $btn.html(orig).prop('disabled', false);
        }
    }).fail(() => {
        toastr.warning('Giriş yapmanız gerekiyor');
        $btn.html(orig).prop('disabled', false);
        setTimeout(() => window.location.href = '/account/login', 800);
    });
}

// ── Wishlist ──
function toggleWishlist(productId, btn) {
    const $btn = $(btn);
    $.post('/wishlist/toggle', { productId }, function (res) {
        if (res.success) {
            $btn.toggleClass('active', res.added);
            $btn.find('i').toggleClass('bi-heart-fill', res.added).toggleClass('bi-heart', !res.added);
            toastr[res.added ? 'success' : 'info'](res.message);
        } else {
            toastr.warning(res.message || 'Giriş yapmanız gerekiyor');
            if (res.redirect) window.location.href = res.redirect;
        }
    });
}

// ── Cart Qty ──
function updateQty(cartItemId, qty) {
    if (qty < 1) return removeCartItem(cartItemId);
    $.post('/Cart/UpdateQuantity', { cartItemId, quantity: qty }, function (res) {
        if (res.success) location.reload();
        else toastr.error(res.message);
    });
}
function removeCartItem(cartItemId) {
    $.post('/Cart/Remove', { cartItemId }, function (res) {
        if (res.success) location.reload();
        else toastr.error(res.message);
    });
}

// ── Coupon ──
function applyCoupon() {
    const code = $('#couponCode').val().trim();
    if (!code) return;
    $('#applyBtn').prop('disabled', true).text('Uygulanıyor...');
    $.post('/Cart/ApplyCoupon', { code }, function (res) {
        if (res.success) { toastr.success(res.message); location.reload(); }
        else { toastr.error(res.message); $('#applyBtn').prop('disabled', false).text('Uygula'); }
    });
}

// ── Image preview ──
function previewImages(input) {
    const preview = document.getElementById('imagePreview');
    if (!preview) return;
    preview.innerHTML = '';
    [...input.files].forEach(f => {
        const reader = new FileReader();
        reader.onload = e => {
            const div = document.createElement('div');
            div.style.cssText = 'position:relative;width:80px;height:80px';
            div.innerHTML = `<img src="${e.target.result}" style="width:80px;height:80px;object-fit:cover;border-radius:10px;border:2px solid var(--border)">`;
            preview.appendChild(div);
        };
        reader.readAsDataURL(f);
    });
}
