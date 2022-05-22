import http from 'k6/http';
import { isEnvVarTrue } from './utils.js';

const scheme = isEnvVarTrue('USE_HTTP') ? 'http' : 'https';
const hostname = __ENV.HOSTNAME;

if (!hostname) {
  throw new Error('HOSTNAME environment variable must be supplied');
}

const baseUrl = `${scheme}://${hostname}`;

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
