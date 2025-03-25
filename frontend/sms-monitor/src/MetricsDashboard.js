import React, { useState, useEffect } from 'react';

const BASE_URL = process.env.REACT_APP_BASE_URL;
function MetricsDashboard() {
    const [metrics, setMetrics] = useState(null);
    const [phoneFilter, setPhoneFilter] = useState("");
    const [startFilter, setStartFilter] = useState("");
    const [endFilter, setEndFilter] = useState("");
    
    const fetchMetrics = () => {
        if (!phoneFilter && !startFilter && !endFilter) return;
        
        let url = `${BASE_URL}/metrics`;
        const params = new URLSearchParams();
        if (phoneFilter) params.append("phoneNumber", phoneFilter);
        if (startFilter) params.append("start", startFilter);
        if (endFilter) params.append("end", endFilter);
        url += "?" + params.toString();
        
        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => setMetrics(data))
            .catch(err => console.error("Error fetching metrics:", err));
    };

    useEffect(() => {
        const handler = setTimeout(() => {
            fetchMetrics();
        }, 500);

        return () => clearTimeout(handler);
    }, [phoneFilter, startFilter, endFilter]);

    const filteredEvents =
        metrics && metrics.events
            ? metrics.events.filter(evt => !phoneFilter || evt.phoneNumber.includes(phoneFilter))
            : [];

    return (
        <div style={{ padding: "20px" }}>
            <h2>Global Metrics</h2>
            {metrics && <p>Global Messages per Second: {metrics.globalRate?.toFixed(2)}</p>}

            <h2>Account Metrics</h2>
            {metrics && metrics.perNumberRate && (
                <table border="1" cellPadding="8" cellSpacing="0">
                    <thead>
                    <tr>
                        <th>Phone Number</th>
                        <th>Messages per Second</th>
                    </tr>
                    </thead>
                    <tbody>
                    {Object.entries(metrics.perNumberRate).map(([phone, rate]) => (
                        <tr key={phone}>
                            <td>{phone}</td>
                            <td>{rate.toFixed(2)}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
            
            <h2>Search Phone Number Events</h2>
            <div style={{ marginBottom: "20px" }}>
                <label>
                    Phone Number:
                    <input
                        type="text"
                        value={phoneFilter}
                        onChange={(e) => setPhoneFilter(e.target.value)}
                        placeholder="Enter phone number"
                        style={{ marginLeft: "8px" }}
                    />
                </label>
                <br />
                <label>
                    Start Time (ISO):
                    <input
                        type="datetime-local"
                        value={startFilter}
                        onChange={e => setStartFilter(e.target.value)}
                        style={{ marginLeft: "8px" }}
                    />
                </label>
                <br />
                <label>
                    End Time (ISO):
                    <input
                        type="datetime-local"
                        value={endFilter}
                        onChange={e => setEndFilter(e.target.value)}
                        style={{ marginLeft: "8px" }}
                    />
                </label>
                <br />
                <button onClick={fetchMetrics} style={{ marginTop: "10px" }}>
                    Apply Filters
                </button>
            </div>

            <h2>Per Number</h2>
            {metrics && metrics.events && (
                <table border="1" cellPadding="8" cellSpacing="0">
                    <thead>
                    <tr>
                        <th>Phone Number</th>
                        <th>Timestamp</th>
                    </tr>
                    </thead>
                    <tbody>
                    {filteredEvents.map((evt, idx) => (
                        <tr key={idx}>
                            <td>{evt.phoneNumber}</td>
                            <td>
                                {new Date(evt.timestamp).toLocaleString('en-US', {
                                    dateStyle: 'medium',
                                    timeStyle: 'short'
                                })}
                            </td>

                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default MetricsDashboard;
