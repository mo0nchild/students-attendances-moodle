import type { GroupInfoModel } from "@models/group-models";
import type { UserInfoModel, UserInfoWithGroupsModel } from "@models/user-models";
import { ArrowUpLeft } from "lucide-react";
import { useMemo, useState, type JSX } from "react";
import { Image, ListGroup } from "react-bootstrap";

interface Props {
	students: UserInfoWithGroupsModel[]
}

interface GroupWithStudents {
  group: GroupInfoModel;
  students: UserInfoModel[];
}

export default function StudentsList({ students }: Props): JSX.Element {
	const groups = useMemo(() => {
		const groupMap: Map<number, GroupWithStudents> = new Map();
		students.forEach(user => {
			user.groups?.forEach(group => {
				if (!groupMap.has(group.externalId)) {
					groupMap.set(group.externalId, { group, students: [] });
				}
				groupMap.get(group.externalId)!.students.push(user);
			});
		});

		return Array.from(groupMap.values());
	}, [ students ])
	const [ current, setCurrent ] = useState<GroupWithStudents>()

  return (
		<div>
			<h5 className='fw-normal text-start fs-6'>Список групп / студентов:</h5>
			{ !groups || groups.length <= 0 
				? <div className='w-100'>
						<h6 className='text-center fs-6 fw-normal'>Список групп пуст</h6>
					</div>
				: <></> }
			{
				!current 
					? <ListGroup className="">
						{
							groups.map((item, index) => (
								<ListGroup.Item key={`teacher-${index}`} onClick={() => setCurrent(item)}
									style={{
										cursor: 'pointer',
										backgroundColor: 'transparent'
									}}
								>
									<div className='d-flex flex-row justify-content-between'>
										<div className='d-flex flex-row align-items-center gap-3'>
											<div>
												<span>{item.group.groupName}</span>
											</div>
											<div>
												<span>{item.group.description}</span>
											</div>
										</div>
										<div>
											<span>Кол-во: {item.students.length}</span>
										</div>
									</div>
								</ListGroup.Item>
							))
						}
						</ListGroup>
					: <div>
							<div className='d-flex flex-row gap-2 align-items-center mb-2'>
								<ArrowUpLeft size={20} onClick={() => setCurrent(undefined)} style={{cursor: 'pointer'}}/>
								<span>{current.group.groupName}</span>
							</div>
							<ListGroup className="">
							{
								current.students.map((item, index) => (
									<ListGroup.Item key={`students-${index}`}
										style={{
											cursor: 'default',
											backgroundColor: 'transparent'
										}}
									>
										<div className='d-flex flex-row align-items-center gap-3'>
											<div>
												<Image src={item.imageUrl} roundedCircle width={30} height={30} />
											</div>
											<div>
												<span>{item.firstName} {item.lastName}</span>
											</div>
											<div>
												<span>{item.email}</span>
											</div>
										</div>
									</ListGroup.Item>
								))
							}
							</ListGroup>
							</div>
			}
		</div>
	)
}