import * as d3 from 'd3';

interface AlbumSalesData {
  month: string;
  albumsSold: number;
  year: number;
}

/**
 * Creates a bar chart visualizing album sales over months.
 * @param containerId - The ID of the HTML container where the chart will be rendered.
 * @param dataUrl - The URL to fetch the album sales data from (expects JSON format).
 */
export async function createAlbumSalesChart(
  containerId: string,
  dataUrl: string
): Promise<void> {
  // Load data from external JSON source
  const data: AlbumSalesData[] = (await d3.json(dataUrl)) || [];

  // Set up dimensions and margins
  const margin = { top: 20, right: 30, bottom: 40, left: 50 };
  const width = 800 - margin.left - margin.right;
  const height = 400 - margin.top - margin.bottom;

  // Create SVG container
  const svg = d3
    .select(`#${containerId}`)
    .append('svg')
    .attr('width', width + margin.left + margin.right)
    .attr('height', height + margin.top + margin.bottom)
    .append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`);

  // Create scales
  const xScale = d3
    .scaleBand()
    .domain(data.map((d) => d.month))
    .range([0, width])
    .padding(0.1);

  const yScale = d3
    .scaleLinear()
    .domain([0, d3.max(data, (d) => d.albumsSold) || 0])
    .nice()
    .range([height, 0]);

  // Add X axis
  svg
    .append('g')
    .attr('transform', `translate(0,${height})`)
    .call(d3.axisBottom(xScale))
    .selectAll('text')
    .attr('transform', 'rotate(-45)')
    .style('text-anchor', 'end');

  // Add Y axis
  svg.append('g').call(d3.axisLeft(yScale));

  // Add X axis label
  svg
    .append('text')
    .attr('x', width / 2)
    .attr('y', height + margin.bottom)
    .style('text-anchor', 'middle')
    .text('Month');

  // Add Y axis label
  svg
    .append('text')
    .attr('transform', 'rotate(-90)')
    .attr('x', -height / 2)
    .attr('y', -margin.left + 15)
    .style('text-anchor', 'middle')
    .text('Albums Sold');

  // Create bars
  svg
    .selectAll('.bar')
    .data(data)
    .enter()
    .append('rect')
    .attr('class', 'bar')
    .attr('x', (d) => xScale(d.month) || 0)
    .attr('y', (d) => yScale(d.albumsSold))
    .attr('width', xScale.bandwidth())
    .attr('height', (d) => height - yScale(d.albumsSold))
    .attr('fill', 'steelblue')
    .on('mouseover', function (event, d) {
      d3.select(this).attr('fill', 'orange');
      
      // Show tooltip
      svg
        .append('text')
        .attr('class', 'tooltip')
        .attr('x', (xScale(d.month) || 0) + xScale.bandwidth() / 2)
        .attr('y', yScale(d.albumsSold) - 5)
        .attr('text-anchor', 'middle')
        .text(`${d.albumsSold} albums`);
    })
    .on('mouseout', function () {
      d3.select(this).attr('fill', 'steelblue');
      svg.selectAll('.tooltip').remove();
    });
}
