import PageCard from "@components/pagecard/PageCard";
import type { JSX } from "react";
import { Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function NotFoundPage(): JSX.Element {
	const navigate = useNavigate()

	return (
		<PageCard>
			<div className="w-100 p-4 p-md-5 pb-md-5 shadow rounded bg-white bg-opacity-75">
				<div className="d-flex flex-column gap-4 align-items-center">
					<h3>Страница не найдена</h3>
					<div className="gradient-input-wrapper" style={{width: 'fit-content'}}>
						<Button className="border-0 bg-transparent" onClick={() => navigate('/dashboard')}>
							Вернуться назад
						</Button>
					</div>
				</div>
			</div>
		</PageCard>
	)
}