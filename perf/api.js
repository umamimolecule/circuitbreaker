import http from 'k6/http';

const baseUrl = 'http://localhost:7071';

const clearEndpoint = {
  method: 'delete',
  path: '/api/status',
};

const endpoints = [
  {
    method: 'get',
    path: '/api/status/{key}',
  },
  {
    method: 'post',
    path: '/api/calls/{key}/success',
  },
  {
    method: 'post',
    path: '/api/calls/{key}/failure',
  },
];

function clearCircuitBreaker() {
  http.request(clearEndpoint.method, getUrl(clearEndpoint.path));
}

function getUrl(path) {
  return `${baseUrl}${path}`;
}

module.exports = {
  baseUrl,
  endpoints,
  clearCircuitBreaker,
  getUrl,
};
