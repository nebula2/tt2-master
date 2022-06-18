export function downloadAssetVersionAsZip(containerReference, version) {
    var link = document.createElement('a');
    link.href = 'Asset/GetAssetZipFile/' + encodeURIComponent(containerReference) + '/' + encodeURIComponent(version);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}