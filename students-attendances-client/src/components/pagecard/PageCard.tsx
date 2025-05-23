import type { JSX } from "react";
import { Col, Container, Row } from "react-bootstrap";

interface Props {
  children: React.ReactNode
  sm?: number
  md?: number
  lg?: number
}

export default function PageCard({ children, sm=12, md=10, lg=10 }: Props): JSX.Element {
	return (
		<Container fluid='sm' className="my-3" style={{
			overflowY: 'auto'
		}}>
			<Row className='justify-content-center'>
				<Col sm={sm} md={md} lg={lg}>{children}</Col>
			</Row>
		</Container>
	)
}