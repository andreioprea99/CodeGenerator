import http from 'k6/http';
import { check } from 'k6';


var payload = JSON.parse(open('TestRequests/complete_test.json'));


export default function () {
    const url = 'https://localhost:44391/api/GenerateCode';
    const headers = { 'Content-Type': 'application/json' };

    const res = http.post(url, JSON.stringify(payload), { headers: headers, timeout: '180s' });

    check(res, { 'status was 201': (r) => r.status == 201 });
}
