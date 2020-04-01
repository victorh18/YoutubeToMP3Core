import axios from 'axios';
const youtubeService = {
    getMetadata,
    getPlaylistMeta,
    download,
    downloadPlaylist
};

function getMetadata(url = '') {
    const encoded = encodeURIComponent(url);
    return axios.get(`${process.env.REACT_APP_API_URL}/Meta?url=${encoded}`).then(resp => resp.data);
}

function getPlaylistMeta(url = '') {
    const encoded = encodeURIComponent(url);
    return axios.get(`${process.env.REACT_APP_API_URL}/Meta/Playlist?url=${encoded}`).then(resp => resp.data);
}

function download(url = '', format = 2) {
    const encoded = encodeURIComponent(url);
    window.open(`${process.env.REACT_APP_API_URL}/Download?url=${encoded}&format=${format}`,'_blank');
}

function downloadPlaylist(url = '', format = 2) {
    const encoded = encodeURIComponent(url);
    window.open(`${process.env.REACT_APP_API_URL}/Download/Playlist?url=${encoded}&format=${format}`,'_blank');
}

export default youtubeService;